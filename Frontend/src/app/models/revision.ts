import { GenericSearchParams } from './pagination';
import { RevisionStatus } from './guitar';

export interface RevisionMetadataExcerpt {
  id: number;
  comment: string;
  status: RevisionStatus;
  createdAt: null | string;
  updatedAt: null | string;
  createdBy: null | string;
  version: null | number;
  parentRevisionId?: number;
}

export interface RevisionMetadata extends RevisionMetadataExcerpt {
  entityType: string;
}

export interface RevisionMetadataWithEntityIdentifier extends RevisionMetadata {
  entityName: string;
  entityId?: string;
}

export enum RevisionStatusEnum {
  Draft = 1,
  Pending = 2,
  Rejected = 3,
  Committed = 4,
}

type RevisionChange<T> = {
  old: T | null;
  new: T;
};

interface BaseRevisionChanges {
  id: RevisionChange<number>;
  createdAt: RevisionChange<string | null>;
  createdBy: RevisionChange<string | null>;
  version: RevisionChange<number | null>;
  comment: RevisionChange<string>;
}

type EntityRevisionChanges<T> = {
  [K in keyof T]?: RevisionChange<T[K]>;
};

export type RevisionChanges<T> = BaseRevisionChanges & EntityRevisionChanges<T>;

export interface RevisionDiff<T> {
  entityId: null | number;
  changes: RevisionChanges<T>;
  iteration: number;
}

export interface UserRevisionSearchParams extends GenericSearchParams {
  status?: string;
}
