import { Component, input, computed, ChangeDetectionStrategy } from '@angular/core';
import { IssueTypeEnum } from '../../enums/issue-type.enum';
import { FaIconComponent } from '../../fa-icon/fa-icon.component';

@Component({
  selector: 'shared-enums-icon-type',
  templateUrl: './enums-icon-type.component.html',
  styleUrls: ['./enums-icon-type.component.css'],
  standalone: true,
  imports: [FaIconComponent],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EnumsIconTypeComponent {
  type = input.required<IssueTypeEnum>();

  src = computed(() => {
    switch (this.type()) {
      case IssueTypeEnum.story: return 'bookmark';
      case IssueTypeEnum.task: return 'check-square';
      case IssueTypeEnum.bug: return 'bug';
      case IssueTypeEnum.knowledge: return 'book';
      case IssueTypeEnum.meeting: return 'handshake';
      default: return '';
    }
  });

  color = computed(() => {
    switch (this.type()) {
      case IssueTypeEnum.story: return 'green';
      case IssueTypeEnum.task: return 'blue';
      case IssueTypeEnum.bug: return 'red';
      case IssueTypeEnum.knowledge: return 'brown';
      case IssueTypeEnum.meeting: return 'orange';
      default: return 'black';
    }
  });

  tooltipTxt = computed(() => {
    switch (this.type()) {
      case IssueTypeEnum.story: return 'Story';
      case IssueTypeEnum.task: return 'Task';
      case IssueTypeEnum.bug: return 'Bug';
      case IssueTypeEnum.knowledge: return 'Knowledge base';
      case IssueTypeEnum.meeting: return 'Meeting';
      default: return '';
    }
  });
}
