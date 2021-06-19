/** Мета данные о роботе */
export class RoboControlModel {
  /** GUID  Идентификатор робота, выдаётся при регистрации */
  public roboId: string;

  /** Время отправки сообщения */
  public timeJs: number;

  public w: boolean;
  public s: boolean;
  public a: boolean;
  public d: boolean;

  public clearControls() {
    this.w = false;
    this.s = false;
    this.a = false;
    this.d = false;
  }
}
