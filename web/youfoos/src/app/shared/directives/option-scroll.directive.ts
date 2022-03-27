import { Directive, EventEmitter, Input, Output,  OnDestroy } from '@angular/core';
import { MatAutocomplete } from '@angular/material/autocomplete';
import { Subject } from 'rxjs';
import { tap, takeUntil } from 'rxjs/operators';

export interface IAutoCompleteScrollEvent {
  autoComplete: MatAutocomplete;
  scrollEvent: Event;
}

@Directive({ selector: 'mat-autocomplete[optionsScroll]' })
export class OptionsScrollDirective implements OnDestroy {

  @Input() thresholdPercent = .8;

  @Output('optionsScroll') scroll = new EventEmitter<IAutoCompleteScrollEvent>();

  _onDestroy = new Subject();

  constructor(public autoComplete: MatAutocomplete) {
    this.autoComplete.opened.pipe(
      tap(() => {
        // Note: When autocomplete raises opened, panel is not yet created (by Overlay)
        // Note: The panel will be available on next tick
        // Note: The panel wil NOT open if there are no options to display
        setTimeout(() => {
          this.autoComplete.panel.nativeElement.addEventListener('scroll', this.onScroll.bind(this));
        });
      }),
      takeUntil(this._onDestroy)).subscribe();

    this.autoComplete.closed.pipe(takeUntil(this._onDestroy)).subscribe();
  }

  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  onScroll(event: Event) {
    if (this.thresholdPercent === undefined) {
      this.scroll.next({ autoComplete: this.autoComplete, scrollEvent: event });
    } else {
      // @ts-ignore
      const threshold = this.thresholdPercent * 100 * event.target.scrollHeight / 100;
      // @ts-ignore
      const current = (<MatAutocomplete>event.target).scrollTop + event.target.clientHeight;

      if (current > threshold) {
        this.scroll.next({ autoComplete: this.autoComplete, scrollEvent: event });
      }
    }
  }

}
