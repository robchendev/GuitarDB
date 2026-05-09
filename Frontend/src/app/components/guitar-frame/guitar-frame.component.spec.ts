import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuitarFrameComponent } from './guitar-frame.component';

describe('GuitarFrameComponent', () => {
  let component: GuitarFrameComponent;
  let fixture: ComponentFixture<GuitarFrameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuitarFrameComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuitarFrameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
