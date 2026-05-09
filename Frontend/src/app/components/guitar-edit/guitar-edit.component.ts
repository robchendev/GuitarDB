import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  Guitar,
  GuitarOriginalData,
  GuitarRevisionSnapshot,
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
import { VALID_URL_REGEX } from '../../constants/regex.constants';
import { requireAllControlsValidator } from '../../utils/form.utils';
import { HotToastService } from '@ngxpert/hot-toast';
import { GuitarsFrameService } from '../../services/guitar-frame.service';

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
  guitarsFrameService = inject(GuitarsFrameService);
  router = inject(Router);
  toast = inject(HotToastService);
  id: number | null = null;
  loadingService = inject(LoadingService);
  guitar?: GuitarOriginalData;

  private fb = new FormBuilder();

  editGuitarForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  submitted = false;
  coordinateGroup = ['latitude', 'longitude'];

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
          const {
            name,
            alias,
            locationAddress,
            locationCoordinates,
            websiteUrl,
            description,
          } = guitar;
          this.editGuitarForm.patchValue({
            comment: '',
            name: name ?? '',
            alias: alias ?? '',
            locationAddress: locationAddress ?? '',
            websiteUrl: websiteUrl ?? '',
            description: description ?? '',
            latitude: locationCoordinates?.latitude,
            longitude: locationCoordinates?.longitude,
          });
        },
      });
  }

  initializeForm() {
    this.editGuitarForm = this.fb.group(
      {
        comment: ['', [Validators.required, Validators.maxLength(300)]],
        name: ['', [Validators.required, Validators.maxLength(100)]],
        alias: ['', [Validators.maxLength(200)]],
        locationAddress: ['', [Validators.maxLength(500)]],
        latitude: ['', [Validators.min(-90), Validators.max(90)]],
        longitude: ['', [Validators.min(-180), Validators.max(180)]],
        websiteUrl: [
          '',
          [Validators.maxLength(300), Validators.pattern(VALID_URL_REGEX)],
        ],
        description: ['', [Validators.maxLength(2000)]],
      },
      {
        validators: requireAllControlsValidator(this.coordinateGroup),
      },
    );
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
      alias: this.editGuitarForm.value.alias ?? undefined,
      locationAddress: this.editGuitarForm.value.locationAddress ?? undefined,
      locationCoordinateLatitude:
        this.editGuitarForm.value.latitude != null &&
        this.editGuitarForm.value.latitude !== ''
          ? Number(this.editGuitarForm.value.latitude)
          : undefined,
      locationCoordinateLongitude:
        this.editGuitarForm.value.longitude != null &&
        this.editGuitarForm.value.longitude !== ''
          ? Number(this.editGuitarForm.value.longitude)
          : undefined,
      websiteUrl: this.editGuitarForm.value.websiteUrl ?? undefined,
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
    const { id, distanceInKilometers, locationCoordinates, ...originalData } =
      this.guitar;

    const normalizedFormValue = {
      ...this.editGuitarForm.value,
      latitude: String(this.editGuitarForm.value.latitude),
      longitude: String(this.editGuitarForm.value.longitude),
    };

    const normalizedOriginalData = {
      ...originalData,
      latitude: String(locationCoordinates?.latitude),
      longitude: String(locationCoordinates?.longitude),
      description: originalData.description ?? '',
    };

    return objectsAreIdentical(normalizedFormValue, normalizedOriginalData);
  }
}
