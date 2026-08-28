import { Service, signal } from '@angular/core';

@Service()
export class BusyService {
    busyRequestCount = signal(0);

    busy() {
        this.busyRequestCount.update(current => current + 1);
    }

    idle() {
        this.busyRequestCount.update(current => Math.max(0, current - 1));
    }
}
