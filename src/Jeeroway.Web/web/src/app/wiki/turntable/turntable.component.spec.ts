import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TurntableComponent } from './turntable.component';

describe('TurntableComponent', () => {
  let component: TurntableComponent;
  let fixture: ComponentFixture<TurntableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TurntableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TurntableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
