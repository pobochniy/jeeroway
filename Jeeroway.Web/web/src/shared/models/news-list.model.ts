import { NewsModel } from './news.model';

export class NewsListModel {
  newsList: NewsModel[] = [];
  page: number = 1;
  totalCount: number = 0;
  pageSize: number = 10;
}
