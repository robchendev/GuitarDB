import { Component, input } from '@angular/core';
import { DiffPart, getDiffParts } from '../../utils/diff.utils';

@Component({
  selector: '[app-revision-diff-row]',
  imports: [],
  templateUrl: './revision-diff-row.component.html',
  styleUrl: './revision-diff-row.component.scss',
})
export class RevisionDiffRowComponent {
  key = input.required<string>();

  // supports both primitives and objects
  value = input<{ old: any; new: any }>();

  // used for primitive fallback display
  oldEmptyFallback = input<string>('');
  newEmptyFallback = input<string>('');

  // enables character-level diffing
  runDiff = input<boolean>(false);

  // NEW: tells component which field to display for object diffs
  displayKey = input<string>(''); // e.g. "name"

  private extract(val: any): any {
    if (!val) return null;

    // if displayKey is provided, assume object diff
    if (this.displayKey()) {
      return val?.[this.displayKey()];
    }

    return val;
  }

  get oldDiffParts(): DiffPart[] {
    const oldVal = this.extract(this.value()?.old);
    const newVal = this.extract(this.value()?.new);

    if (!this.runDiff()) {
      return [{ value: oldVal ?? this.oldEmptyFallback() }];
    }

    return getDiffParts(false, oldVal, newVal, this.oldEmptyFallback());
  }

  get newDiffParts(): DiffPart[] {
    const oldVal = this.extract(this.value()?.old);
    const newVal = this.extract(this.value()?.new);

    if (!this.runDiff()) {
      return [{ value: newVal ?? this.newEmptyFallback() }];
    }

    return getDiffParts(true, oldVal, newVal, this.newEmptyFallback());
  }
}
