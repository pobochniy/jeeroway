import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyPrompt } from './survey-prompt';

describe('SurveyPrompt', () => {
  let component: SurveyPrompt;
  let fixture: ComponentFixture<SurveyPrompt>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SurveyPrompt]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SurveyPrompt);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
