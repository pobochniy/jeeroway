import { FormControl, FormGroup, Validators } from "@angular/forms";

/** Модель на странице входа */
export let roboFormModel = new FormGroup({
  /** Идентификатор робота, выдаётся при регистрации */
  'id': new FormControl(null, null),

  /** Наименование робота */
  'name': new FormControl(null, [Validators.required, Validators.minLength(3)]),

  /** Краткое описание робота */
  'description': new FormControl(null, null),

  /** Принадлежность к рою */
  //'swarm': new FormControl('', null),
});
