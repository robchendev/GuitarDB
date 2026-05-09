import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RevisionMetadataExcerpt } from '../../models/revision';
import { GuitarsService } from '../../services/guitars.service';
import { getReadableDate } from '../../utils/date.utils';
import { HttpResponse } from '@angular/common/http';
import { getEntityBaseUrlFromEntityPath } from '../../utils/entity.utils';
import { GuitarsFrameService } from '../../services/guitar-frame.service';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { createPaginationSignals, FormKeyMap } from '../../utils/params.utils';
import { PaginatedDirectoryComponent } from '../abstract/paginated-directory/paginated-directory.component';
import {
  requireAllControlsValidator,
  allControlsGroupFilled,
} from '../../utils/form.utils';
import { PaginatedResult } from '../../models/pagination';

@Component({
  selector: 'app-revision-history',
  imports: [RouterLink, PaginationControlsComponent],
  templateUrl: './revision-history.component.html',
  styleUrl: './revision-history.component.scss',
})
export class RevisionHistoryComponent extends PaginatedDirectoryComponent<
  RevisionMetadataExcerpt,
  GuitarsService
> {
  protected service = inject(GuitarsService);
  protected items = signal<RevisionMetadataExcerpt[]>([]);
  protected loadingKey = '';
  protected formKeyMap: FormKeyMap = {}; // intentionally empty

  guitarsFrameService = inject(GuitarsFrameService);
  entityId?: number;
  entityPath?: string;
  entityName?: string;
  private coordinateControlNames = ['latitude', 'longitude'];

  constructor() {
    super();

    // This component can be part of any Guitar/Grinder/etc frame, so get generic id instead of relying on Entity Frames
    const id = Number(this.route.parent?.snapshot.paramMap.get('id'));
    if (id === undefined || isNaN(id)) return;

    const parentUrlSegments = this.route.parent?.snapshot.url;
    if (!parentUrlSegments?.length) {
      console.error('base route path not found');
      return;
    }
    this.entityPath = parentUrlSegments[0].path;
    this.loadingKey = `${this.entityPath}-revision-history-${id}`;
    this.entityId = id;
  }

  protected override getFormGroupValidators() {
    return {
      validators: requireAllControlsValidator(this.coordinateControlNames),
    };
  }

  protected fetchPaginatedItems() {
    if (!this.entityId) return;
    switch (this.entityPath) {
      case 'guitars':
        return this.service.getGuitarRevisionMetadataExcerpts(
          {
            page: this.paginationSignals.page.signal(),
            pageSize: this.paginationSignals.pageSize.signal(),
          },
          this.entityId,
          true,
        );
      default:
        console.error('Unhandled entity path', this.entityPath);
        return;
    }
  }

  override fetchPaginatedItemsNext(
    res: HttpResponse<RevisionMetadataExcerpt[]>,
    result: PaginatedResult<RevisionMetadataExcerpt[]>,
  ) {
    if (!res) return;
    this.entityName = res.headers.get('Guitar-Name') || '';
  }

  get coordinateGroupHasError(): boolean {
    return allControlsGroupFilled(this.searchForm, this.coordinateControlNames);
  }

  getReadableDate(dateIsoString: string | null): string | null {
    return getReadableDate(dateIsoString);
  }

  getEntityBaseUrl(entityName: string) {
    return getEntityBaseUrlFromEntityPath(entityName);
  }
}
