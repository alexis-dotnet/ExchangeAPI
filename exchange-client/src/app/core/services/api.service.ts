import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../../environments/environment';
import {PurchaseRequestDto, PurchaseResponseDto, RateResponseDto} from '../models/exchange.models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) {
  }

  private getHttpHeaders(): any {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
  }

  getRate(currencyCode: string): Observable<any> {
    const httpOptions = this.getHttpHeaders();
    console.log(environment.apiUrl);

    return this.http
      .get<RateResponseDto>(`${environment.apiUrl}/api/exchange/rate/${currencyCode}`, httpOptions)
      .pipe();
  }

  postPurchase(request: PurchaseRequestDto): Observable<any> {
    const httpOptions = this.getHttpHeaders();
    console.log(environment.apiUrl);

    return this.http
      .post<PurchaseResponseDto>(`${environment.apiUrl}/api/purchases`, request, httpOptions)
      .pipe();
  }
}
