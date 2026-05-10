import { Component, inject, OnInit } from '@angular/core';
import { LoadingService } from '../../services/loading.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { GuitarsService } from '../../services/guitars.service';
import { GuitarRevisionSnapshot, GuitarFields } from '../../models/guitar';
import { RevisionDiffRowComponent } from '../revision-diff-row/revision-diff-row.component';
import { getMultilineDiffParts } from '../../utils/diff.utils';
import { getReadableDate } from '../../utils/date.utils';
import { GuitarsFrameService } from '../../services/guitar-frame.service';
import { AccountService } from '../../services/account.service';
import { RevisionDiff } from '../../models/revision';

@Component({
  selector: 'app-guitar-revision',
  imports: [RevisionDiffRowComponent, RouterLink],
  templateUrl: './guitar-revision.component.html',
  styleUrls: [
    '../revision-diff-row/revision-diff-row.component.scss',
    './guitar-revision.component.scss',
  ],
})
export class GuitarRevisionComponent implements OnInit {
  private guitarsService = inject(GuitarsService);
  accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  guitarId: number | null = null;
  loadingService = inject(LoadingService);
  loadingKey = '';
  guitarsFrameService = inject(GuitarsFrameService);

  guitarRevisionDiff?: Omit<RevisionDiff<GuitarFields>, 'iteration'>;

  ngOnInit(): void {
    // Not using guitarsFrameService to load cache in here, since I dont expect users to go back and forth between revisions
    // and even if they do, the revisions would get replaced anyways, so caching wouldn't do anything.
    // If we want to implement caching in the future, we should do a queryKey mapped caching
    this.loadGuitarRevisionSnapshot();
  }

  loadGuitarRevisionSnapshot() {
    const { paramMap } = this.route.snapshot;
    const revisionId1 = Number(paramMap.get('revisionId1'));
    let revisionId2: number | null = Number(paramMap.get('revisionId2'));
    let guitarId = this.guitarsFrameService.guitarId();

    this.loadingKey = `revision-${guitarId}-${revisionId1}-${revisionId2}`;
    this.loadingService.busy(this.loadingKey);
    if (revisionId2 && guitarId) {
      this.guitarsService
        .getGuitarRevisionDiffs(guitarId, revisionId1, revisionId2)
        .subscribe({
          next: (guitarRevisionDiff: RevisionDiff<GuitarFields>) => {
            this.guitarRevisionDiff = guitarRevisionDiff;
            if (this.guitarRevisionDiff.changes['createdAt']) {
              this.guitarRevisionDiff.changes['createdAt'].old =
                this.getReadableDate(
                  this.guitarRevisionDiff.changes['createdAt'].old,
                );
              this.guitarRevisionDiff.changes['createdAt'].new =
                this.getReadableDate(
                  this.guitarRevisionDiff.changes['createdAt'].new,
                );
            }
            this.loadingService.idle(this.loadingKey);
          },
          error: () => {
            this.loadingService.idle(this.loadingKey);
          },
        });
    } else {
      this.guitarsService.getGuitarRevisionSnapshot(revisionId1).subscribe({
        next: (guitarRevisionSnapshot: GuitarRevisionSnapshot) => {
          if (guitarRevisionSnapshot.parentRevisionId != null) return;

          const {
            guitarId: rId,
            id,
            createdAt,
            createdBy,
            version,
            comment,
            name,
            description,
          } = guitarRevisionSnapshot;
          this.guitarRevisionDiff = {
            entityId: rId,
            changes: {
              id: {
                old: null,
                new: id,
              },
              createdAt: {
                old: null,
                new: createdAt ? this.getReadableDate(createdAt) : null,
              },
              createdBy: {
                old: null,
                new: createdBy ?? null,
              },
              version: {
                old: null,
                new: version ?? null,
              },
              comment: {
                old: null,
                new: comment,
              },
              // properties that are only added in diff if they are changed
              name:
                name == null
                  ? undefined
                  : {
                      old: null,
                      new: name,
                    },
              description:
                description == null
                  ? undefined
                  : {
                      old: null,
                      new: description,
                    },
            },
          };
          this.loadingService.idle(this.loadingKey);
        },
        error: () => {
          this.loadingService.idle(this.loadingKey);
        },
      });
    }
  }

  getEntries() {
    if (!this.guitarRevisionDiff) return [];
    return Object.entries(this.guitarRevisionDiff.changes);
  }

  getParagraphs(description: string): string[] {
    if (!description) {
      return [];
    }
    return description.split('\n').filter((p) => p.trim().length > 0);
  }

  getReadableDate(dateIsoString: unknown | null): string | null {
    if (typeof dateIsoString !== 'string' && dateIsoString !== null) {
      return null;
    }
    return getReadableDate(dateIsoString);
  }

  public getMultilineDiffParts(
    isNewDiff: boolean,
    oldVal?: string | null,
    newVal?: string | null,
    fallback: string = '',
  ) {
    return getMultilineDiffParts(isNewDiff, oldVal, newVal, fallback);
  }
}
