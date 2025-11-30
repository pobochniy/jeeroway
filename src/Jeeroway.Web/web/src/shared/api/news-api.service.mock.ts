import {Injectable} from '@angular/core';
import {INewsApiService} from './news-api.service';
import {NewsListModel} from '../models/news-list.model';
import {NewsModel} from '../models/news.model';
import {NewsShowModel} from '../models/news-show.model';

@Injectable()
export class NewsApiServiceMock implements INewsApiService {
  async getList(page: number, pageSize: number): Promise<NewsListModel> {
    const all: NewsModel[] = [
      {
        id: '1',
        alias: 'open-beta',
        title: 'Открытое бета-тестирование StarCombats началось!',
        text: 'Приготовьтесь к бою! Наше долгожданное открытое бета-тестирование теперь доступно для всех игроков. Погрузитесь в захватывающие космические баталии, исследуйте новые галактики и станьте частью нашей вселенной...',
        date: '10.09.2025'
      },
      {
        id: '2',
        alias: 'scout-class',
        title: 'Представляем новый класс кораблей: "Разведчик"!',
        text: 'Встречайте, "Разведчик" — наш новый, невероятно быстрый и маневренный класс кораблей. Он создан для тех, кто предпочитает тактику внезапных атак и скрытых операций в тылу врага. Его уникальные способности помогут вам получить преимущество в любой схватке.',
        date: '08.09.2025'
      },
      {
        id: '3',
        alias: 'tournament-announcement',
        title: 'Грандиозный турнир "Галактическое Превосходство" — скоро!',
        text: 'Готовьте свои ангары и оттачивайте мастерство! Мы анонсируем первый крупный турнир "Галактическое Превосходство" с призовым фондом в $10000. Вся информация о правилах и регистрации будет опубликована на нашем сайте в ближайшие дни.',
        date: '05.09.2025'
      }
    ];

    const model = new NewsListModel();
    model.totalCount = all.length;
    model.pageSize = pageSize;
    model.newsList = all.slice((page - 1) * pageSize, (page - 1) * pageSize + pageSize);
    return model;
  }

  async getNewsById(id: string): Promise<NewsShowModel> {
    // Return a stubbed item; id is alias in our case
    return {
      id: id,
      alias: id,
      title: 'Новость: ' + id,
      date: '2025-09-10',
      isPublished: true,
      author: 'Admin',
      brief: 'Короткое описание новости',
      description: 'Полное описание новости для редактирования',
      tags: 'star,combats'
    } as NewsShowModel;
  }

  getNewsByAlias(alias: string): Promise<NewsShowModel> {
    return this.getNewsById(alias);
  }

  async saveNews(model: NewsShowModel): Promise<string> {
    // Simulate create/update by returning id (alias-based fake guid)
    return model.id ?? '00000000-0000-0000-0000-000000000001';
  }
}
