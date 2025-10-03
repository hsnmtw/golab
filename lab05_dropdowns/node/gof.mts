//@ts-ignore
Array.prototype.sum = function() : number { 
    return this!.reduce(function(a,x){return a+x;},0); 
};



function xrange(start:number, end: number) /* number[] */ {
    var ans = [];
    for (var i = start; i <= end; i++) {
        ans.push(i);
    }
    return ans;
}


interface GOL {
    getState() : number[][];
    mutate() : void;
}

function GameOfLife(width: number, high : number, callback : (state: number[][]) => void) : GOL {
    if(!width||width===0) width=38
    if(!high||high===0) high=38

    

    let state : number[][] = xrange(0,width).map(_=>xrange(0,high).map(_=>0))
//1
    state[5]![1]=1;
    state[6]![1]=1;
    state[5]![2]=1;
    state[6]![2]=1;



//2
    state[5]![11]=1 //
    state[6]![11]=1 //
    state[7]![11]=1 //

    state[4]![12]=1 //
    state[3]![13]=1 //
    state[3]![14]=1 //

    state[8]![12]=1 //
    state[9]![13]=1 //
    state[9]![14]=1 //

    state[6]![15]=1 //
    
    state[4]![16]=1 //
    state[8]![16]=1 //
    state[5]![17]=1
    state[6]![17]=1
    state[7]![17]=1
    state[6]![18]=1

//3
    state[3]![21]=1
    state[3]![22]=1

    state[4]![21]=1
    state[4]![22]=1

    state[5]![21]=1
    state[5]![22]=1

    state[2]![23]=1
    state[2]![25]=1
    state[1]![25]=1

    state[6]![23]=1
    state[6]![25]=1
    state[7]![25]=1

//4
    state[3]![35]=1
    state[3]![36]=1
    state[4]![35]=1
    state[4]![36]=1

    

    const neighbors /* number[][] */ = [
        [-1,-1],
        [-1, 0],
        [-1,+1],
        [+1,-1],
        [+1, 0],
        [+1,+1],
        [ 0,-1],
        [ 0,+1],
    ]

    function countNeighbors(y: number,x : number) /* number */ {
        const arr = neighbors.map(n => y+n[0]!>-1 
                               && y+n[0]!<state.length 
                               && x+n[1]!>-1 
                               && x+n[1]!<state[y+n[0]!]!.length ? state[y+n[0]!]![x+n[1]!] : 0);
        // @ts-ignore
        return arr.sum();

    }

    function mutate(){
        const generation : number[][] = xrange(0,width).map(_=>xrange(0,high).map(_=>0))
        for(let y=0;y<state.length;y++){
            for(let x=0;x<state[y]!.length;x++){
                const s = state[y]![x]
                const c = countNeighbors(y,x)
                generation[y]![x] = (c===3 ? 1 
                                         : c<2||c>3 
                                              ? 0 
                                              : s) !; 
            }
        }
        if(callback && typeof callback === "function") 
            callback(generation)
        state=generation
    }

    function getState() : number[][]  {
        return state.map(y => y.map(x=>x===0?0:1))
    }

    return {getState,mutate}
}



export {xrange, GameOfLife}