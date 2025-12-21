import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RpiInstallComponent } from './rpi-install.component';

describe('RpiInstallComponent', () => {
  let component: RpiInstallComponent;
  let fixture: ComponentFixture<RpiInstallComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RpiInstallComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RpiInstallComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
