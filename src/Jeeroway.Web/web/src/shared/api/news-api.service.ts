import {Injectable} from '@angular/core';
import {BaseApiService} from './base-api.service';
import {HttpClient} from '@angular/common/http';
import {TokenService} from './token.service';
import {NewsListModel} from '../models/news-list.model';
import {NewsShowModel} from '../models/news-show.model';

export interface INewsApiService {
  getList(page: number, pageSize: number): Promise<NewsListModel>;

  getNewsById(id: string): Promise<NewsShowModel>;
  getNewsByAlias(alias: string): Promise<NewsShowModel>;

  saveNews(model: NewsShowModel): Promise<string>;
}

@Injectable()
export class NewsApiService extends BaseApiService implements INewsApiService {
  constructor(http: HttpClient, tokenService: TokenService) {
    super('News', tokenService, http);
  }

  async getList(page: number, pageSize: number): Promise<NewsListModel> {
    // Assuming backend endpoint supports pagination via query params
    const url = `GetList?page=${page}&pageSize=${pageSize}`;
    // toPromise style to align with project
    // eslint-disable-next-line rxjs/no-topromise
    const res = await this.get<NewsListModel>(url).toPromise();
    return res ?? new NewsListModel();
  }

  async getNewsById(id: string): Promise<NewsShowModel> {
    const url = `GetById?id=${id}`;
    // eslint-disable-next-line rxjs/no-topromise
    const res = await this.get<NewsShowModel>(url).toPromise();
    return res ?? new NewsShowModel();
  }

  async getNewsByAlias(id: string): Promise<NewsShowModel> {
    const url = `GetByAlias?alias=${encodeURIComponent(id)}`;
    // eslint-disable-next-line rxjs/no-topromise
    const res = await this.get<NewsShowModel>(url).toPromise();
    return res ?? new NewsShowModel();
  }

  async saveNews(model: NewsShowModel): Promise<string> {
    // eslint-disable-next-line rxjs/no-topromise
    await this.post<void>('Save', model).toPromise();
    return model.id!;
  }
}
