import { Component, inject } from '@angular/core';
import { GuitarEditComponent } from '../guitar-edit/guitar-edit.component';
import { ActivatedRoute } from '@angular/router';
import { GuitarRevisionSnapshot } from '../../models/guitar';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { TextAreaComponent } from '../forms/text-area/text-area.component';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-guitar-revision-edit',
  imports: [
    FormCtaButtonComponent,
    TextAreaComponent,
    TextInputComponent,
    ReactiveFormsModule,
  ],
  templateUrl: './guitar-revision-edit.component.html',
  styleUrls: [
    '../guitar-edit/guitar-edit.component.scss',
    './guitar-revision-edit.component.scss',
  ],
})
export class GuitarRevisionEditComponent extends GuitarEditComponent {
  private route = inject(ActivatedRoute);
  revisionId?: number;

  override loadGuitar() {
    this.loadGuitarRevision();
  }

  override updateGuitar() {
    this.updateGuitarRevision();
  }

  loadGuitarRevision() {
    // load guitar revision instead
    const revisionIdFromRoute = Number(
      this.route.snapshot.paramMap.get('revisionId'),
    );
    if (isNaN(revisionIdFromRoute)) return;

    this.guitarsService
      .getGuitarRevisionSnapshot(revisionIdFromRoute)
      .subscribe({
        next: (guitarRevisionSnapshot: GuitarRevisionSnapshot) => {
          const { id, name, description, luthier, comment, guitarId } =
            guitarRevisionSnapshot;

          this.revisionId = id;

          // original state
          this.guitar = {
            comment,
            id: guitarId ?? 0, // this doesn't matter for editing the revision, but we want typescript to be happy
            name,
            description,
            luthier,
          };
          this.editGuitarForm.patchValue({
            comment: comment ?? '',
            name: name ?? '',
            description: description ?? '',
            luthier: luthier ?? '',
          });
        },
      });
  }

  override updateGuitarNext = (
    guitarRevisionSnapshot: GuitarRevisionSnapshot,
    loadingId: string,
  ) => {
    this.validationErrors = [];
    this.loadingService.idle(loadingId);

    const guitarId = guitarRevisionSnapshot.guitarId;

    if (!guitarId) return;

    this.router.navigate([
      '/guitars',
      guitarId,
      'revisions',
      guitarRevisionSnapshot.id,
      guitarRevisionSnapshot.parentRevisionId,
    ]);

    this.toast.success(
      `Guitar revision ID ${guitarRevisionSnapshot.id} successfully created! It will be reviewed shortly.`,
    );
  };

  updateGuitarRevision() {
    if (!this.revisionId) return;

    const loadingId = `edit-guitar-revision-${this.revisionId}`;
    this.loadingService.busy(loadingId);
    this.guitarsService
      .updateGuitarRevision(this.revisionId, this.getUpdatePayload())
      .subscribe({
        next: (guitarRevisionSnapshot: GuitarRevisionSnapshot) => {
          this.updateGuitarNext(guitarRevisionSnapshot, loadingId);
          this.router.navigateByUrl('/guitars');
          this.router.navigate([
            '/guitars/new/revisions',
            guitarRevisionSnapshot.id,
          ]);
          this.toast.success(
            `Guitar revision ID ${guitarRevisionSnapshot.id} successfully updated! It will be reviewed by the moderation team shortly for Guitar Creation.`,
          );
        },
        error: (error) => {
          this.updateGuitarError(error, loadingId);
        },
      });
  }
}
