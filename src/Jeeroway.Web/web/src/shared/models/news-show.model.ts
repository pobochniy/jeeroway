export class NewsShowModel {
  id?: string | null;
  alias: string = '';
  title: string = '';
  date?: string | null; // ISO string or formatted date
  isPublished: boolean = false;
  author: string = '';
  brief: string = '';
  description: string = '';
  tags?: string | null;
}
