import { HtmlTagDefinition } from '@angular/compiler';
import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { ConfirmDialogueService } from '../../core/services/confirm-dialogue-service';

@Component({
  selector: 'app-confirm-dialogue',
  imports: [],
  templateUrl: './confirm-dialogue.html',
  styleUrl: './confirm-dialogue.css',
})
export class ConfirmDialogue {
  @ViewChild('dialogueRef') dialogueRef!: ElementRef<HTMLDialogElement>;
  message = 'Are you sure?';
  private resolver: ((result: boolean) => void) | null = null;

  constructor() {
    inject(ConfirmDialogueService).register(this);
  }

  open(message: string) {
    this.message = message;
    this.dialogueRef.nativeElement.showModal();
    return new Promise<boolean>(resolve => (this.resolver = resolve));
  }

  confirm() {
    this.dialogueRef.nativeElement.close();
    this.resolver?.(true);
    this.resolver = null;
  }

  cancel() {
    this.dialogueRef.nativeElement.close();
    this.resolver?.(false);
    this.resolver = null;
  }
}
