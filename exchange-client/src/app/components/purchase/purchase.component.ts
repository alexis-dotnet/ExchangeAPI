import {Component, OnInit} from '@angular/core';
import {PurchaseRequestDto, PurchaseResponseDto, RateResponseDto} from '../../core/models/exchange.models';
import {ApiService} from '../../core/services/api.service';
import {FormControl} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {catchError} from 'rxjs/internal/operators';
import {throwError} from 'rxjs';

@Component({
  selector: 'app-purchase',
  templateUrl: './purchase.component.html',
  styleUrls: ['./purchase.component.css']
})
export class PurchaseComponent implements OnInit {
  rateResponseUsd: RateResponseDto = {currency: '', sell: null, buy: null, rateDate: null};
  rateResponseBrl: RateResponseDto = {currency: '', sell: null, buy: null, rateDate: null};
  purchaseResponse: PurchaseResponseDto = {currency: '', purchasedAmount: 0};

  userIdControl = new FormControl(1);
  amountControl = new FormControl(0);
  currencyControl = new FormControl('USD');

  constructor(private apiService: ApiService, private toastr: ToastrService) {
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

  submit(): void {
    const request: PurchaseRequestDto = {
      userId: this.userIdControl.value,
      originAmount: this.amountControl.value,
      targetCurrency: this.currencyControl.value
    };

    this.apiService
      .postPurchase(request)
      .pipe(
        catchError(error => {
          this.toastr.error(error.error.detail, 'Failure');
          return throwError(error);
        })
      )
      .subscribe((response: PurchaseResponseDto) => {
        this.purchaseResponse = response;
        this.toastr.success(`You have just bought ${response.purchasedAmount} ${response.currency}`, 'Success!');
      });
  }
}
