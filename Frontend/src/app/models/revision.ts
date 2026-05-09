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

export interface UserRevisionSearchParams extends GenericSearchParams {
  status?: string;
}
