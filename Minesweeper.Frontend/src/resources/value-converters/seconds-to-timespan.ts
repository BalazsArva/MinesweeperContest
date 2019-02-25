export class SecondsToTimespanValueConverter {
  toView(totalSeconds: number) {
    totalSeconds = totalSeconds || 0;

    let seconds = totalSeconds % 60;

    let totalMinutes = Math.floor(totalSeconds / 60);
    let minutes = totalMinutes % 60;

    let totalHours = Math.floor(totalMinutes / 60);

    let secondsFormatted = seconds.toString().padStart(2, '0');
    let minutesFormatted = minutes.toString().padStart(2, '0');
    let hoursFormatted = totalHours.toString().padStart(2, '0');

    return `${hoursFormatted}:${minutesFormatted}:${secondsFormatted}`;
  }

  fromView(value) {

  }
}

