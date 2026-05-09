import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  CreateGuitarDto,
  Guitar,
  GuitarRevisionDiff,
  GuitarRevisionSnapshot,
  GuitarSearchParams,
  UpdateGuitarDto,
} from '../models/guitar';
import { getHttpParams } from '../utils/params.utils';
import { RevisionMetadataExcerpt } from '../models/revision';
import { GenericSearchParams } from '../models/pagination';

@Injectable({
  providedIn: 'root',
})
export class GuitarsService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getGuitars(guitarSearchParams: GuitarSearchParams) {
    return this.http.get<Guitar[]>(`${this.baseUrl}guitars/`, {
      observe: 'response',
      params: getHttpParams(guitarSearchParams),
    });
  }

  getGuitar(id: number) {
    return this.http.get<Guitar>(`${this.baseUrl}guitars/${id}`);
  }

  createGuitar(createGuitarDto: CreateGuitarDto) {
    return this.http.post<GuitarRevisionSnapshot>(
      `${this.baseUrl}guitars/create`,
      createGuitarDto,
    );
  }

  updateGuitar(id: number, createGuitarRevisionDto: UpdateGuitarDto) {
    return this.http.post<GuitarRevisionSnapshot>(
      `${this.baseUrl}guitars/${id}/create-revision`,
      createGuitarRevisionDto,
    );
  }

  onDeleteGuitar(id: number) {
    return this.http.delete<void>(`${this.baseUrl}guitars/${id}`);
  }

  getGuitarRevisionSnapshot(id: number) {
    return this.http.get<GuitarRevisionSnapshot>(
      `${this.baseUrl}guitars/revisions/${id}`,
    );
  }

  updateGuitarRevision(
    revisionId: number,
    createGuitarRevisionDto: UpdateGuitarDto,
  ) {
    return this.http.patch<GuitarRevisionSnapshot>(
      `${this.baseUrl}guitars/revisions/${revisionId}/edit`,
      createGuitarRevisionDto,
    );
  }

  getGuitarRevisionDiffs(
    guitarId: number,
    revisionId1: number,
    revisionId2: number | null,
  ) {
    return this.http.get<GuitarRevisionDiff>(
      `${this.baseUrl}guitars/${guitarId}/revisions/diff?revisionId1=${revisionId1}&revisionId2=${revisionId2}`,
    );
  }

  getGuitarRevisionMetadataExcerpts(
    searchParams: GenericSearchParams,
    guitarId: number,
    committedOnly: boolean,
  ) {
    return this.http.get<RevisionMetadataExcerpt[]>(
      `${this.baseUrl}guitars/${guitarId}/revisions${committedOnly ? '?committedOnly=true' : ''}`,
      {
        observe: 'response',
        params: getHttpParams(searchParams),
      },
    );
  }
}
