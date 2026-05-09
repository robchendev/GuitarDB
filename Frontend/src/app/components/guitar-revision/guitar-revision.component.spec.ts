import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuitarRevisionComponent } from './guitar-revision.component';

describe('GuitarRevisionComponent', () => {
  let component: GuitarRevisionComponent;
  let fixture: ComponentFixture<GuitarRevisionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuitarRevisionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuitarRevisionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
