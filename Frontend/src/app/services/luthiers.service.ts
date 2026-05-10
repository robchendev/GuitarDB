import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  CreateLuthierDto,
  Luthier,
  LuthierFields,
  LuthierRevisionSnapshot,
  LuthierSearchParams,
  UpdateLuthierDto,
} from '../models/luthier';
import { getHttpParams } from '../utils/params.utils';
import { RevisionDiff, RevisionMetadataExcerpt } from '../models/revision';
import { GenericSearchParams } from '../models/pagination';

@Injectable({
  providedIn: 'root',
})
export class LuthiersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getLuthiers(luthierSearchParams: LuthierSearchParams) {
    return this.http.get<Luthier[]>(`${this.baseUrl}luthiers/`, {
      observe: 'response',
      params: getHttpParams(luthierSearchParams),
    });
  }

  getLuthier(id: number) {
    return this.http.get<Luthier>(`${this.baseUrl}luthiers/${id}`);
  }

  createLuthier(createLuthierDto: CreateLuthierDto) {
    return this.http.post<LuthierRevisionSnapshot>(
      `${this.baseUrl}luthiers/create`,
      createLuthierDto,
    );
  }

  updateLuthier(id: number, createLuthierRevisionDto: UpdateLuthierDto) {
    return this.http.post<LuthierRevisionSnapshot>(
      `${this.baseUrl}luthiers/${id}/create-revision`,
      createLuthierRevisionDto,
    );
  }

  onDeleteLuthier(id: number) {
    return this.http.delete<void>(`${this.baseUrl}luthiers/${id}`);
  }

  getLuthierRevisionSnapshot(id: number) {
    return this.http.get<LuthierRevisionSnapshot>(
      `${this.baseUrl}luthiers/revisions/${id}`,
    );
  }

  updateLuthierRevision(
    revisionId: number,
    createLuthierRevisionDto: UpdateLuthierDto,
  ) {
    return this.http.patch<LuthierRevisionSnapshot>(
      `${this.baseUrl}luthiers/revisions/${revisionId}/edit`,
      createLuthierRevisionDto,
    );
  }

  getLuthierRevisionDiffs(
    luthierId: number,
    revisionId1: number,
    revisionId2: number | null,
  ) {
    return this.http.get<RevisionDiff<LuthierFields>>(
      `${this.baseUrl}luthiers/${luthierId}/revisions/diff?revisionId1=${revisionId1}&revisionId2=${revisionId2}`,
    );
  }

  getLuthierRevisionMetadataExcerpts(
    searchParams: GenericSearchParams,
    luthierId: number,
    committedOnly: boolean,
  ) {
    return this.http.get<RevisionMetadataExcerpt[]>(
      `${this.baseUrl}luthiers/${luthierId}/revisions${committedOnly ? '?committedOnly=true' : ''}`,
      {
        observe: 'response',
        params: getHttpParams(searchParams),
      },
    );
  }
}
