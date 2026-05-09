import { Injectable, signal } from '@angular/core';
import { RevisionMetadataExcerpt } from '../models/revision';
import { Guitar } from '../models/guitar';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root',
})
export class GuitarsFrameService {
  guitarId = signal<number | null>(null);
  guitar = signal<Guitar | null>(null);

  reset() {
    this.guitar.set(null);
  }
}
