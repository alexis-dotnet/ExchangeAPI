export interface RateResponseDto {
  currency: string;
  buy?: number;
  sell?: number;
  rateDate?: Date;
}

export interface PurchaseRequestDto {
  userId: number;
  originAmount: number;
  targetCurrency: string;
}

export interface PurchaseResponseDto {
  currency: string;
  purchasedAmount: number;
}
