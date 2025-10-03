import { GameOfLife, xrange } from "./gof.mjs";
import modernizeSelect from "./select.mjs";
modernizeSelect('select');
const gof = document.querySelector('.gof');
const grid = xrange(0, 38).map(_ => xrange(0, 38).map(_ => { }));
for (let row = 0; row < 12; row++) {
    grid.push([]);
    for (let col = 0; col < 38; col++) {
        const cell = document.createElement('i');
        gof.append(cell);
        grid[row][col] = cell;
        cell.innerHTML = '&nbsp;';
        cell.style.fontSize = '5pt';
    }
}
var game = GameOfLife(0, 0, state => {
    for (let row = 0; row < 38; row++) {
        for (let col = 0; col < 38; col++) {
            //console.log(row,col)
            //console.log(grid)
            if (grid[row][col] && grid[row][col].style)
                grid[row][col].style.backgroundColor = state[row][col] == 0 ? '' : '#fff';
        }
    }
});
setTimeout(() => { setInterval(game.mutate, 100); }, 1000);
