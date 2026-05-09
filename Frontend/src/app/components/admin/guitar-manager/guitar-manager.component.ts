import { Component, inject } from '@angular/core';
import { GuitarDirectoryComponent } from '../../guitar-directory/guitar-directory.component';
import { TextInputComponent } from '../../forms/text-input/text-input.component';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';
import { ReactiveFormsModule } from '@angular/forms';
import { ErrorTextComponent } from '../../error-text/error-text.component';
import { RouterLink } from '@angular/router';
import { Guitar } from '../../../models/guitar';
import { HotToastService } from '@ngxpert/hot-toast';
import { SimpleModalComponent } from '../../modal/modal.component';

@Component({
  selector: 'app-guitar-manager2',
  imports: [
    TextInputComponent,
    PaginationControlsComponent,
    ReactiveFormsModule,
    ErrorTextComponent,
    RouterLink,
    SimpleModalComponent,
  ],
  templateUrl: './guitar-manager.component.html',
  styleUrls: [
    '../../guitar-directory/guitar-directory.component.scss',
    '../../abstract/paginated-directory/paginated-directory.component.scss',
    './guitar-manager.component.scss',
  ],
})
export class GuitarManagerComponent extends GuitarDirectoryComponent {
  private toast = inject(HotToastService);
  selectedGuitar: Guitar | null = null;
  deletedGuitarIds = new Set<number>();
  override loadingKey = 'guitar-manager';

  showModal(guitar: Guitar) {
    this.selectedGuitar = { ...guitar }; // shallow copy to avoid messing up the original
  }

  hideModal() {
    this.selectedGuitar = null;
  }

  onDeleteGuitar() {
    if (!this.selectedGuitar) return;
    const { name, id } = this.selectedGuitar;
    const loadingId = `delete-guitar-${id}`;
    this.loadingService.busy(loadingId);

    this.service.onDeleteGuitar(id).subscribe({
      next: () => {
        const deletedGuitar = this.paginatedResultSignal()?.items?.find(
          (r: Guitar) => r.id === id,
        );
        if (!deletedGuitar) {
          this.toast.success(
            `Guitar ${name} not found in client side memory, this should not happen`,
          );
          return;
        }
        this.deletedGuitarIds.add(id);
        this.toast.success(`Guitar '${name}' deleted`);
        this.loadingService.idle(loadingId);
        this.hideModal();
      },
      error: (error: any) => {
        console.error(error);
        this.toast.error(error);
        this.loadingService.idle(loadingId);
        this.hideModal();
      },
    });
  }

  onEditNavigate(id: number) {
    this.router.navigate(['/guitars/edit', id]);
  }
}
