import {Component, OnInit} from '@angular/core';
import {ApiService} from '../../core/services/api.service';
import {RateResponseDto} from '../../core/models/exchange.models';

@Component({
  selector: 'app-quote',
  templateUrl: './quote.component.html',
  styleUrls: ['./quote.component.css']
})
export class QuoteComponent implements OnInit {
  rateResponseUsd: RateResponseDto = {currency: '', sell: null, buy: null, rateDate: null};
  rateResponseBrl: RateResponseDto = {currency: '', sell: null, buy: null, rateDate: null};

  constructor(private apiService: ApiService) {
  }

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.apiService
      .getRate('USD')
      .pipe()
      .subscribe((response: RateResponseDto) => {
        this.rateResponseUsd = response;
      });
    this.apiService
      .getRate('BRL')
      .pipe()
      .subscribe((response: RateResponseDto) => {
        this.rateResponseBrl = response;
      });
  }

}
