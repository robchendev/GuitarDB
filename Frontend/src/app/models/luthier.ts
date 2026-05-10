export interface LuthierBase {
  name: string;
  description: null | string;
  luthier: null | LuthierExcerpt;
}

export interface LuthierExcerpt {
  id: number;
  name: string;
}

export interface Luthier extends LuthierBase {
  id: number;
}

export interface LuthierOriginalData extends Luthier {
  comment: string;
}

export interface CreateLuthierDto {
  comment: string;
  name: string;
  description?: string;
}

export interface UpdateLuthierDto {
  comment: string;
  name: string;
  description?: string;
}

export interface LuthierSearchParams {
  page?: number;
  pageSize?: number; // refactor page and pageSize
  name?: string;
}

export type RevisionStatus = 'Draft' | 'Pending' | 'Rejected' | 'Committed'; // refactor

export interface LuthierRevisionSnapshot extends LuthierBase {
  id: number;
  luthierId: number | null;
  version: number | null;
  parentRevisionId: number | null;
  comment: string;
  status: RevisionStatus;
  createdAt: string | null;
  createdBy: string;
  updatedAt: string | null;
  updatedBy: string;
}

export interface LocationCoordinates {
  latitude: number;
  longitude: number;
}
export interface LuthierFields {
  name: string;
  description?: string;
}
