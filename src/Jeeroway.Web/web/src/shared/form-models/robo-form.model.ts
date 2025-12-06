import { FormControl, FormGroup, Validators } from "@angular/forms";

/** Модель на странице входа */
export const roboFormModel: FormGroup = new FormGroup({
  /** Идентификатор робота, выдаётся при регистрации */
  'id': new FormControl<string | null>(null),

  /** Наименование робота */
  'name': new FormControl<string>('', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]),

  /** Краткое описание робота */
  'description': new FormControl<string | null>(null),

  /** Принадлежность к рою */
  //'swarm': new FormControl<string>('', []),
});
