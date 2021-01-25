import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {AppComponent} from './app.component';
import {AppRoutingModule} from './app-routing.module';
import {QuoteComponent} from './components/quote/quote.component';
import {PurchaseComponent} from './components/purchase/purchase.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ToastrModule} from 'ngx-toastr';
import {NgxSpinnerModule} from 'ngx-spinner';
import {LoaderInterceptor} from './core/interceptors/loader.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    QuoteComponent,
    PurchaseComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    CommonModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(), // ToastrModule added
    NgxSpinnerModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoaderInterceptor,
      multi: true,
    },

  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
