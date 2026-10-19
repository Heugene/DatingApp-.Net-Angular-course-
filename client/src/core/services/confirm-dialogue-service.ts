import { Service } from '@angular/core';
import { ConfirmDialogue } from '../../shared/confirm-dialogue/confirm-dialogue';

@Service()
export class ConfirmDialogueService {
    private dialogueComponent?: ConfirmDialogue;

    register(component: ConfirmDialogue) {
        this.dialogueComponent = component;
    }

    confirm(message = 'Are you sure?'): Promise<boolean> {
        if (!this.dialogueComponent) {
            throw new Error('Confirm dialogue component is not registered');
        }
        return this.dialogueComponent.open(message);
    }
}
