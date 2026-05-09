import { Component, inject, signal } from '@angular/core';
import { FormKeyMap, sanitizeObjectFields } from '../../utils/params.utils';
import { GuitarsService } from '../../services/guitars.service';
import { RouterLink } from '@angular/router';
import { AuthDirective } from '../../directive/auth.directive';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { Guitar } from '../../models/guitar';
import { ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import {
  allControlsGroupFilled,
  requireAllControlsValidator,
} from '../../utils/form.utils';
import { ErrorTextComponent } from '../error-text/error-text.component';
import { PaginatedDirectoryComponent } from '../abstract/paginated-directory/paginated-directory.component';

@Component({
  selector: 'app-guitars',
  imports: [
    PaginationControlsComponent,
    AuthDirective,
    RouterLink,
    TextInputComponent,
    ReactiveFormsModule,
    ErrorTextComponent,
  ],
  templateUrl: './guitar-directory.component.html',
  styleUrls: [
    '../abstract/paginated-directory/paginated-directory.component.scss',
    './guitar-directory.component.scss',
  ],
})
export class GuitarDirectoryComponent extends PaginatedDirectoryComponent<
  Guitar,
  GuitarsService
> {
  protected service = inject(GuitarsService);
  protected items = signal<Guitar[]>([]);
  protected loadingKey = 'guitar-directory';
  protected formKeyMap: FormKeyMap = {
    name: {
      paramCode: 'n',
      default: '',
      validators: [Validators.maxLength(100)],
    },
    address: {
      paramCode: 'a',
      default: '',
      validators: [Validators.maxLength(200)],
    },
    lat: {
      paramCode: 'la',
      default: '',
      validators: [Validators.min(-90), Validators.max(90)],
    },
    long: {
      paramCode: 'lo',
      default: '',
      validators: [Validators.min(-180), Validators.max(180)],
    },
    radius: {
      paramCode: 'r',
      default: '',
      validators: [Validators.min(0.1), Validators.max(15000)],
    },
  };

  private coordinateControlNames = ['lat', 'long', 'radius'];

  protected override getFormGroupValidators() {
    return {
      validators: requireAllControlsValidator(this.coordinateControlNames),
    };
  }

  protected fetchPaginatedItems() {
    return this.service.getGuitars({
      page: this.paginationSignals.page.signal(),
      pageSize: this.paginationSignals.pageSize.signal(),
      ...sanitizeObjectFields(this.searchForm.value),
    });
  }

  get isDistanceProvided(): boolean {
    return this.items().some(
      (guitar) => guitar.distanceInKilometers !== null,
    );
  }

  get coordinateGroupHasError(): boolean {
    return allControlsGroupFilled(this.searchForm, this.coordinateControlNames);
  }
}
