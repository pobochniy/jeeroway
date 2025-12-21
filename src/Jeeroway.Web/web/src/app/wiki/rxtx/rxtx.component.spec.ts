import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RxtxComponent } from './rxtx.component';

describe('RxtxComponent', () => {
  let component: RxtxComponent;
  let fixture: ComponentFixture<RxtxComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RxtxComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RxtxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
