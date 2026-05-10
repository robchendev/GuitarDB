import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  Guitar,
  GuitarOriginalData,
  GuitarRevisionSnapshot,
  LuthierExcerpt,
} from '../../models/guitar';
import { LoadingService } from '../../services/loading.service';
import { GuitarsService } from '../../services/guitars.service';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { objectsAreIdentical } from '../../utils/objects.utils';
import { TextAreaComponent } from '../forms/text-area/text-area.component';
import { HotToastService } from '@ngxpert/hot-toast';
import { GuitarsFrameService } from '../../services/guitar-frame.service';
import { LuthiersService } from '../../services/luthiers.service';
import { Luthier } from '../../models/luthier';
import { HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-guitar-edit',
  imports: [
    ReactiveFormsModule,
    TextInputComponent,
    FormCtaButtonComponent,
    TextAreaComponent,
  ],
  templateUrl: './guitar-edit.component.html',
  styleUrl: './guitar-edit.component.scss',
})
export class GuitarEditComponent implements OnInit {
  protected guitarsService = inject(GuitarsService);
  protected luthiersService = inject(LuthiersService);
  guitarsFrameService = inject(GuitarsFrameService);
  router = inject(Router);
  toast = inject(HotToastService);
  id: number | null = null;
  loadingService = inject(LoadingService);
  guitar?: GuitarOriginalData;
  luthierSearchResults: Luthier[] = [];
  selectedLuthier: LuthierExcerpt | null = null;

  private fb = new FormBuilder();

  editGuitarForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  submitted = false;

  ngOnInit(): void {
    this.initializeForm();
    // Wont be using guitarFrameService for caching in here, we'd best show the most up to date defaults to the user
    this.loadGuitar();
  }

  loadGuitar() {
    let guitarId = this.guitarsFrameService.guitarId();
    if (!guitarId) return;

    this.guitarsService
      .getGuitar(Number(this.guitarsFrameService.guitarId()))
      .subscribe({
        next: (guitar: Guitar) => {
          this.guitar = {
            ...guitar,
            comment: '',
          };
          const { name, description, luthier } = guitar;
          this.selectedLuthier = luthier;
          this.editGuitarForm.patchValue({
            comment: '',
            name: name ?? '',
            description: description ?? '',
            luthier: luthier?.name ?? '',
            luthierId: luthier?.id ?? '',
          });
        },
      });
  }

  initializeForm() {
    this.editGuitarForm = this.fb.group({
      comment: ['', [Validators.required, Validators.maxLength(300)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(2000)]],
      luthierDisplayName: [''],
      luthier: ['', [Validators.maxLength(100)]],
      luthierId: [null],
    });
  }

  onEditGuitar() {
    this.submitted = true;

    if (!this.editGuitarForm.valid) {
      this.editGuitarForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    if (this.editGuitarForm.pristine || this.getIsDataUnchanged()) {
      this.validationErrors = ['No changes were made to the guitar details.'];
      return;
    }
    this.updateGuitar();
  }

  onSearchLuthier() {
    this.luthiersService
      .getLuthiers({
        page: 1,
        pageSize: 5,
        name: this.editGuitarForm.value.luthier,
      })
      .subscribe({
        next: (response: HttpResponse<Luthier[]>) => {
          this.luthierSearchResults = response.body ?? [];
        },
      });
  }

  onAssignLuthier(luthier: Luthier) {
    this.editGuitarForm.patchValue({
      luthier: luthier.name,
      luthierId: luthier.id,
    });
    this.selectedLuthier = {
      name: luthier.name,
      id: luthier.id,
    };
    this.luthierSearchResults = [];
  }

  updateGuitar() {
    const guitarId = this.guitarsFrameService.guitarId();
    if (guitarId == null || guitarId == undefined) {
      this.validationErrors = ['Guitar ID not found, is the URL correct?'];
      return;
    }
    const loadingId = `edit-guitar-${guitarId}`;
    this.loadingService.busy(loadingId);
    this.guitarsService
      .updateGuitar(guitarId, this.getUpdatePayload())
      .subscribe({
        next: (guitarRevisionSnapshot: GuitarRevisionSnapshot) => {
          this.updateGuitarNext(guitarRevisionSnapshot, loadingId);
        },
        error: (error) => {
          this.updateGuitarError(error, loadingId);
        },
      });
  }

  getUpdatePayload() {
    return {
      comment: this.editGuitarForm.value.comment,
      name: this.editGuitarForm.value.name,
      luthierId: this.editGuitarForm.value.luthierId ?? undefined,
      description: this.editGuitarForm.value.description ?? undefined,
    };
  }

  updateGuitarNext = (
    guitarRevisionSnapshot: GuitarRevisionSnapshot,
    loadingId: string,
  ) => {
    this.validationErrors = [];
    this.loadingService.idle(loadingId);
    this.router.navigate([
      '/guitars',
      this.guitar?.id ?? 'new',
      'revisions',
      guitarRevisionSnapshot.id,
      guitarRevisionSnapshot.parentRevisionId,
    ]);
    this.toast.success(
      `Guitar revision ID ${guitarRevisionSnapshot.id} successfully created! It will be reviewed by the moderation team shortly for Guitar Update.`,
    );
  };

  updateGuitarError = (error: any, loadingId: string) => {
    this.loadingService.idle(loadingId);
    this.validationErrors = [error];
  };

  getIsDataUnchanged(): boolean {
    if (!this.guitar) return true;
    const { id, ...originalData } = this.guitar;

    const normalizedFormValue = {
      ...this.editGuitarForm.value,
    };

    const normalizedOriginalData = {
      ...originalData,
      description: originalData.description ?? '',
    };

    return objectsAreIdentical(normalizedFormValue, normalizedOriginalData);
  }
}
