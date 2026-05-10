import { Component, inject, signal } from '@angular/core';
import { FormKeyMap, sanitizeObjectFields } from '../../utils/params.utils';
import { GuitarsService } from '../../services/guitars.service';
import { RouterLink } from '@angular/router';
import { AuthDirective } from '../../directive/auth.directive';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { Guitar } from '../../models/guitar';
import { ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { PaginatedDirectoryComponent } from '../abstract/paginated-directory/paginated-directory.component';

@Component({
  selector: 'app-guitars',
  imports: [
    PaginationControlsComponent,
    AuthDirective,
    RouterLink,
    TextInputComponent,
    ReactiveFormsModule,
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
  };

  protected fetchPaginatedItems() {
    return this.service.getGuitars({
      page: this.paginationSignals.page.signal(),
      pageSize: this.paginationSignals.pageSize.signal(),
      ...sanitizeObjectFields(this.searchForm.value),
    });
  }
}
