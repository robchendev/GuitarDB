import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuitarRevisionEditComponent } from './guitar-revision-edit.component';

describe('GuitarRevisionEditComponent', () => {
  let component: GuitarRevisionEditComponent;
  let fixture: ComponentFixture<GuitarRevisionEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuitarRevisionEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuitarRevisionEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
