import { Component, effect, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingService } from '../../services/loading.service';
import { GuitarsService } from '../../services/guitars.service';
import { Guitar } from '../../models/guitar';
import { GuitarsFrameService } from '../../services/guitar-frame.service';

@Component({
  selector: 'app-guitar-details',
  imports: [],
  templateUrl: './guitar-details.component.html',
  styleUrl: './guitar-details.component.scss',
})
export class GuitarDetailsComponent {
  private guitarsService = inject(GuitarsService);
  guitarsFrameService = inject(GuitarsFrameService);
  private router = inject(Router);
  private id: number | null = null;
  loadingService = inject(LoadingService);
  loadingKey = '';

  constructor() {
    this.loadingKey = `guitars-${this.guitarsFrameService.guitar()?.id}`;

    effect(() => {
      const guitarId = this.guitarsFrameService.guitarId();
      const guitar = this.guitarsFrameService.guitar();

      if (guitarId && !guitar) {
        this.loadingService.busy(this.loadingKey);
        this.guitarsService.getGuitar(guitarId).subscribe({
          next: (guitar: Guitar) => {
            this.guitarsFrameService.guitar.set(guitar);
            this.loadingService.idle(this.loadingKey);
          },
          error: () => {
            this.loadingService.idle(this.loadingKey);
          },
        });
      }
    });
  }

  onNavigateAddGuitar() {
    if (this.id == null) return;
    this.router.navigate(['/guitars', this.id, 'edit']);
  }

  get descriptionParagraphs(): string[] {
    const guitar = this.guitarsFrameService.guitar();
    if (!guitar?.description) {
      return [];
    }
    return guitar.description.split('\n').filter((p) => p.trim().length > 0);
  }
}
