import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {TokenService} from "./token.service";

export class BaseApiService {
  private readonly baseUrl;

  constructor(
    public apiName: string,
    public tokenService: TokenService,
    public http: HttpClient
  ) {
    this.baseUrl = '/api/' + apiName + '/';
  }

  public get<T>(url: string): Observable<T> {
    return this.http.get<T>(this.baseUrl + url, {
      withCredentials: true,
      headers: new HttpHeaders({'Authorization': 'Bearer ' + this.tokenService.Token})
    });
  }

  public post<T>(url: string, data: any = {}): Observable<T> {
    return this.http.post<T>(this.baseUrl + url, data, {
      withCredentials: true,
      headers: new HttpHeaders({'Authorization': 'Bearer ' + this.tokenService.Token})
    });
  }
}
