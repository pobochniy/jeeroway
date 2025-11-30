import { ExpService } from './exp.service';

describe('ExpService', () => {
  let service: ExpService;

  beforeEach(() => {
    service = new ExpService();
  });

  it('should report next up = 20 and next level = 110 when exp = 0', () => {
    const html = service.expHtml(0);

    expect(html).toContain('Опыт: 0');
    expect(html).toContain('До апа : 20');
    expect(html).toContain('До уровня : 110');
  });
});
