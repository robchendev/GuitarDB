export interface GuitarBase {
  name: string;
  alias: null | string;
  locationAddress: null | string;
  locationCoordinates: null | LocationCoordinates;
  websiteUrl: null | string;
  description: null | string;
}

export interface Guitar extends GuitarBase {
  id: number;
  distanceInKilometers?: number;
}

export interface GuitarOriginalData extends Guitar {
  comment: string;
}

export interface CreateGuitarDto {
  comment: string;
  name: string;
  alias?: string;
  locationAddress?: string;
  locationCoordinateLatitude?: number;
  locationCoordinateLongitude?: number;
  websiteUrl?: string;
  description?: string;
}

export interface UpdateGuitarDto {
  comment: string;
  name: string;
  alias?: string;
  locationAddress?: string;
  locationCoordinateLatitude?: number;
  locationCoordinateLongitude?: number;
  websiteUrl?: string;
  description?: string;
}

export interface GuitarSearchParams {
  page?: number;
  pageSize?: number;
  name?: string;
  address?: string;
  lat?: number;
  long?: number;
  radius?: number;
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

export interface GuitarRevisionDiff {
  guitarId: null | number;
  changes: {
    // properties that always exist
    id: {
      old: null | number;
      new: number;
    };
    createdAt: {
      old: null | string;
      new: null | string;
    };
    createdBy: {
      old: null | string;
      new: null | string;
    };
    version: {
      old: null | number;
      new: null | number;
    };
    comment: {
      old: null | string;
      new: string;
    };
    // properties that are only added in diff if they are changed
    name?: {
      old: null | string;
      new: null | string;
    };
    alias?: {
      old: null | string;
      new: null | string;
    };
    locationAddress?: {
      old: null | string;
      new: null | string;
    };
    locationCoordinates?: {
      old: null | LocationCoordinates;
      new: null | LocationCoordinates;
    };
    websiteUrl?: {
      old: null | string;
      new: null | string;
    };
    description?: {
      old: null | string;
      new: null | string;
    };
  };
}
