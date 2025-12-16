import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {MatTabsModule} from '@angular/material/tabs';
import {FormsModule} from '@angular/forms';
import {NgSelectModule} from '@ng-select/ng-select';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatTabsModule, FormsModule, NgSelectModule],
  templateUrl: 'app.html'
})

export class AppComponent {
}
