import { Component, inject, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { GuitarsFrameService } from '../../services/guitar-frame.service';

@Component({
  selector: 'app-guitar-frame',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './guitar-frame.component.html',
  styleUrl: './guitar-frame.component.scss',
})
export class GuitarFrameComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  guitarsFrameService = inject(GuitarsFrameService);

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      const guitarIdFromRoute = params.get('id');
      let guitarId =
        guitarIdFromRoute == null ? undefined : Number(guitarIdFromRoute);

      if (guitarId !== this.guitarsFrameService.guitar()?.id) {
        this.guitarsFrameService.reset();
        if (!isNaN(Number(guitarId))) {
          this.guitarsFrameService.guitarId.set(Number(guitarId));
        } else if (guitarIdFromRoute == 'new') {
          this.guitarsFrameService.guitarId.set(null);
        } else {
          console.error('shouldnt go here');
          return;
        }
      }
    });
  }
}
