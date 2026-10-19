import { Component, inject, OnInit } from '@angular/core';
import { Nav } from "../layout/nav/nav";
import { Router, RouterOutlet } from '@angular/router';
import { ConfirmDialogue } from '../shared/confirm-dialogue/confirm-dialogue';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Nav, RouterOutlet, ConfirmDialogue]
})
export class App {
  protected router = inject(Router);
}
