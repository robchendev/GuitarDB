import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { GuitarRevisionSnapshot } from '../../models/guitar';
import { GuitarsService } from '../../services/guitars.service';
import { LoadingService } from '../../services/loading.service';
import { Router } from '@angular/router';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { TextAreaComponent } from '../forms/text-area/text-area.component';
import { VALID_URL_REGEX } from '../../constants/regex.constants';
import { requireAllControlsValidator } from '../../utils/form.utils';
import { HotToastService } from '@ngxpert/hot-toast';

@Component({
  selector: 'app-guitar-create',
  imports: [
    ReactiveFormsModule,
    TextInputComponent,
    FormCtaButtonComponent,
    TextAreaComponent,
  ],
  templateUrl: './guitar-create.component.html',
  styleUrl: './guitar-create.component.scss',
})
export class GuitarCreateComponent implements OnInit {
  private guitarsService = inject(GuitarsService);
  private fb = new FormBuilder();
  private toast = inject(HotToastService);
  loadingService = inject(LoadingService);
  private router = inject(Router);

  createGuitarForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  submitted = false;
  coordinateGroup = ['latitude', 'longitude'];

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.createGuitarForm = this.fb.group({
      comment: [
        'Initial revision',
        [Validators.required, Validators.maxLength(300)],
      ],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      alias: ['', [Validators.maxLength(200)]],
      locationAddress: ['', [Validators.maxLength(500)]],
      latitude: [
        null,
        [
          Validators.min(-90),
          Validators.max(90),
          requireAllControlsValidator(this.coordinateGroup),
        ],
      ],
      longitude: [
        null,
        [
          Validators.min(-180),
          Validators.max(180),
          requireAllControlsValidator(this.coordinateGroup),
        ],
      ],
      websiteUrl: [
        '',
        [Validators.maxLength(300), Validators.pattern(VALID_URL_REGEX)],
      ],
      description: ['', [Validators.maxLength(2000)]],
    });
  }

  onCreateGuitar() {
    this.submitted = true;

    if (!this.createGuitarForm.valid) {
      this.createGuitarForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.loadingService.busy('create-guitar');
    this.guitarsService
      .createGuitar({
        comment: this.createGuitarForm.value.comment,
        name: this.createGuitarForm.value.name,
        alias: this.createGuitarForm.value.alias ?? undefined,
        locationAddress:
          this.createGuitarForm.value.locationAddress ?? undefined,
        locationCoordinateLatitude:
          this.createGuitarForm.value.latitude != null &&
          this.createGuitarForm.value.latitude !== ''
            ? Number(this.createGuitarForm.value.latitude)
            : undefined,
        locationCoordinateLongitude:
          this.createGuitarForm.value.longitude != null &&
          this.createGuitarForm.value.longitude !== ''
            ? Number(this.createGuitarForm.value.longitude)
            : undefined,
        websiteUrl: this.createGuitarForm.value.websiteUrl ?? undefined,
        description: this.createGuitarForm.value.description ?? undefined,
      })
      .subscribe({
        next: (guitarRevisionSnapshot: GuitarRevisionSnapshot) => {
          this.validationErrors = [];
          this.loadingService.idle('create-guitar');
          this.router.navigateByUrl('/guitars');
          this.router.navigate([
            '/guitars/new/revisions',
            guitarRevisionSnapshot.id,
          ]);
          this.toast.success(
            `Guitar revision ID ${guitarRevisionSnapshot.id} successfully created! It will be reviewed by the moderation team shortly for Guitar Creation.`,
          );
        },
        error: (error) => {
          this.loadingService.idle('create-guitar');
          this.validationErrors = [error];
        },
      });
  }

  get isDataEmpty(): boolean {
    if (!this.createGuitarForm) return true;
    const formValue = this.createGuitarForm.value;

    for (const key in formValue) {
      if (formValue.hasOwnProperty(key)) {
        const value = formValue[key];
        if (typeof value === 'string' && value.trim().length > 0) return false;
        if (
          value !== null &&
          value !== undefined &&
          typeof value !== 'string'
        ) {
          return false;
        }
      }
    }

    return true;
  }
}
