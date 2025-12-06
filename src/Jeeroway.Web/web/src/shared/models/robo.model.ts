/** Мета данные о роботе */
export class RoboModel {
  /** GUID  Идентификатор робота, выдаётся при регистрации */
  public id: string = '';

  /** Наименование робота */
  public name: string = '';

  /** Краткое описание робота */
  public description: string = '';

  /** Идентификатор хозяина */
  public masterId: string = '';
}
