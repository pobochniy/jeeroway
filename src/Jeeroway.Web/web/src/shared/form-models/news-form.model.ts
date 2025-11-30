import {FormGroup, FormControl, Validators} from "@angular/forms";
import {today} from "./today";

export let newsFormModel: FormGroup = new FormGroup({
  id: new FormControl<string | null>(null),
  alias: new FormControl('', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]),
  title: new FormControl('', [Validators.required, Validators.maxLength(500)]),
  date: new FormControl<string | null>(today),
  isPublished: new FormControl(false),
  author: new FormControl(''),
  brief: new FormControl('', [Validators.maxLength(500)]),
  description: new FormControl('', [Validators.required, Validators.maxLength(2000)]),
  tags: new FormControl<string | null>(null, [Validators.maxLength(100)])
});
