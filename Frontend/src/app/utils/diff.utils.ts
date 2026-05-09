import { Change, diffChars, diffWords } from 'diff';
import { LocationCoordinates } from '../models/guitar';

export interface DiffPart {
  value: string;
  type?: 'added' | 'removed' | 'unchanged';
}

export interface ParagraphDiff {
  parts: DiffPart[];
}

export function getDiffHtml(
  isNewDiff: boolean,
  oldVal?: any,
  newVal?: any,
  fallback: string = '',
): string {
  const oldStr = oldVal?.toString() ?? '';
  const newStr = newVal?.toString() ?? '';

  if (!oldStr && !newStr) return fallback;

  const diff = diffWords(oldStr, newStr);

  return diff
    .map((part: Change) => {
      if (isNewDiff) {
        if (part.added) return `<span class="added">${part.value}</span>`;
        if (part.removed) return '';
      } else {
        if (part.removed) return `<span class="removed">${part.value}</span>`;
        if (part.added) return '';
      }
      return part.value;
    })
    .join('');
}

export function getDiffParts(
  isNewDiff: boolean,
  oldVal?: any,
  newVal?: any,
  fallback: string = '',
): DiffPart[] {
  const oldStr = oldVal?.toString() ?? '';
  const newStr = newVal?.toString() ?? '';

  if (!oldStr && !newStr) return [{ value: fallback }];

  const diff = diffWords(oldStr, newStr);

  return diff
    .map((part: Change) => {
      if (isNewDiff) {
        if (part.added) return { value: part.value, type: 'added' };
        if (part.removed) return null;
      } else {
        if (part.removed) return { value: part.value, type: 'removed' };
        if (part.added) return null;
      }
      return { value: part.value };
    })
    .filter((p): p is DiffPart => p !== null);
}

export function getMultilineDiffParts(
  isNewDiff: boolean,
  oldVal?: string | null,
  newVal?: string | null,
  fallback: string = 'None',
): ParagraphDiff[] {
  if (!oldVal && !newVal)
    return [{ parts: [{ value: fallback, type: 'unchanged' }] }];

  const oldParagraphs = oldVal?.split(/\r?\n/) ?? [];
  const newParagraphs = newVal?.split(/\r?\n/) ?? [];
  const maxLen = Math.max(oldParagraphs.length, newParagraphs.length);

  return Array.from({ length: maxLen }).map((_, i) => {
    const oldPara = oldParagraphs[i] ?? '';
    const newPara = newParagraphs[i] ?? '';

    const diff: Change[] = diffWords(oldPara, newPara);

    const parts: DiffPart[] = diff
      .map((part) => {
        if (isNewDiff) {
          if (part.added) return { value: part.value, type: 'added' as const };
          if (part.removed) return { value: '', type: 'unchanged' as const };
        } else {
          if (part.removed)
            return { value: part.value, type: 'removed' as const };
          if (part.added) return { value: '', type: 'unchanged' as const };
        }
        return { value: part.value, type: 'unchanged' as const };
      })
      .filter((p) => p.value !== '');
    return { parts };
  });
}

export function getCoordinatesDiffParts(
  isNewDiff: boolean = false,
  oldCoords?: LocationCoordinates | null,
  newCoords?: LocationCoordinates | null,
  fallback: string = 'None',
): DiffPart[] {
  if (!oldCoords && !newCoords) return [{ value: fallback }];

  const oldLatStr = oldCoords?.latitude.toFixed(6) ?? '';
  const oldLngStr = oldCoords?.longitude.toFixed(6) ?? '';
  const newLatStr = newCoords?.latitude.toFixed(6) ?? '';
  const newLngStr = newCoords?.longitude.toFixed(6) ?? '';

  const highlightDiff = (oldStr: string, newStr: string) => {
    const diff = diffChars(oldStr, newStr);
    return diff
      .map((part: Change) => {
        if (isNewDiff) {
          if (part.added) return { value: part.value, type: 'added' };
          if (part.removed) return null;
        } else {
          if (part.removed) return { value: part.value, type: 'removed' };
          if (part.added) return null;
        }
        return { value: part.value };
      })
      .filter((p): p is DiffPart => p !== null);
  };

  return [
    ...highlightDiff(oldLatStr, newLatStr),
    { value: ', ' },
    ...highlightDiff(oldLngStr, newLngStr),
  ];
}
