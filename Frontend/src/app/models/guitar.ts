export interface GuitarBase {
  name: string;
  description: null | string;
  luthier: null | LuthierExcerpt;
}

export interface LuthierExcerpt {
  id: number;
  name: string;
}

export interface Guitar extends GuitarBase {
  id: number;
}

export interface GuitarOriginalData extends Guitar {
  comment: string;
}

export interface CreateGuitarDto {
  comment: string;
  name: string;
  description?: string;
}

export interface UpdateGuitarDto {
  comment: string;
  name: string;
  description?: string;
}

export interface GuitarSearchParams {
  page?: number;
  pageSize?: number; // refactor page and pageSize
  name?: string;
}

export type RevisionStatus = 'Draft' | 'Pending' | 'Rejected' | 'Committed';

export interface GuitarRevisionSnapshot extends GuitarBase {
  id: number;
  guitarId: number | null;
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
export interface GuitarFields {
  name: string;
  description?: string;
  luthier?: {
    id: number;
    name: string;
  };
}
