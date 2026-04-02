export function LeftDominance(graph) {
    const mB2LinkModel = graph.getCell('mB2Link');
    const mB2LinkChild1Model = graph.getCell('mB2LinkChild1');
    const mB2LinkChild2Model = graph.getCell('mB2LinkChild2');
    const mB2LinkChild3Model = graph.getCell('mB2LinkChild3');
    const mB2LinkChild4Model = graph.getCell('mB2LinkChild4');
    if (!mB2LinkModel) return;

    // 3️⃣ Update the link
    mB2LinkModel.set({
        source: { x: 994, y: 1296 },
        target: { x: 1491, y: 1294 },
        vertices: [
            { x: 1040, y: 1345 }, { x: 1109, y: 1390 }, { x: 1189, y: 1416 }, { x: 1260, y: 1428 }, { x: 1323, y: 1416 },
            { x: 1400, y: 1371 }, { x: 1453, y: 1327 }
        ],
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: 'CIRC AV' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
    mB2LinkChild1Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .67 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '1st LPL' } },
                position: { distance: 0.7, angle: 10 }
            }
        ],
    });
    mB2LinkChild2Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .47 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '2nd LPL' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
    mB2LinkChild3Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .27 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '3rd LPL' } },
                position: { distance: 0.6, angle: 10 }
            }
        ]
    });
    mB2LinkChild4Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .03 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: 'IPDA' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
}
export function RightDominance(graph) {
    const mB2LinkModel = graph.getCell('mB2Link');
    const mB2LinkChild1Model = graph.getCell('mB2LinkChild1');
    const mB2LinkChild2Model = graph.getCell('mB2LinkChild2');
    const mB2LinkChild3Model = graph.getCell('mB2LinkChild3');
    const mB2LinkChild4Model = graph.getCell('mB2LinkChild4');
    if (!mB2LinkModel) return;

    // 3️⃣ Update the link
    mB2LinkModel.set({
        source: { x: 827, y: 1431 },
        target: { x: 1342, y: 1407 },
        vertices: [
            { x: 894, y: 1382 }, { x: 945, y: 1335 }, { x: 994, y: 1296 }, { x: 1040, y: 1345 }, { x: 1109, y: 1390 }, { x: 1189, y: 1416 }, { x: 1260, y: 1428 }
        ],
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: 'rPAV' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
    mB2LinkChild1Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .97 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '1st RPL' } },
                position: { distance: 0.5, angle: 10 }
            }
        ],
    });
    mB2LinkChild2Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .8 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '2nd RPL' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
    mB2LinkChild3Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .63 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '1st RPL' } },
                position: { distance: 0.6, angle: 10 }
            }
        ]
    });
    mB2LinkChild4Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .36 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: 'rPDA' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
}
export function CoDominance(graph) {
    const mB2LinkModel = graph.getCell('mB2Link');
    const mB2LinkChild1Model = graph.getCell('mB2LinkChild1');
    const mB2LinkChild2Model = graph.getCell('mB2LinkChild2');
    const mB2LinkChild3Model = graph.getCell('mB2LinkChild3');
    const mB2LinkChild4Model = graph.getCell('mB2LinkChild4');
    if (!mB2LinkModel) return;

    // 3️⃣ Update the link
    mB2LinkModel.set({
        source: { x: 827, y: 1431 },
        target: { x: 1491, y: 1294 },
        vertices: [
            { x: 894, y: 1382 }, { x: 945, y: 1335 }, { x: 994, y: 1296 }, { x: 1040, y: 1345 }, { x: 1109, y: 1390 }, { x: 1189, y: 1416 }, { x: 1260, y: 1428 }, { x: 1323, y: 1416 },
            { x: 1400, y: 1371 }, { x: 1453, y: 1327 }
        ],
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: 'CIRC AV' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
    mB2LinkChild1Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .75 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '1st LPL' } },
                position: { distance: 0.7, angle: 10 }
            }
        ],
    });
    mB2LinkChild2Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .6 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '2nd LPL' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
    mB2LinkChild3Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .48 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: '3rd LPL' } },
                position: { distance: 0.6, angle: 10 }
            }
        ]
    });
    mB2LinkChild4Model.set({
        source: {
            id: mB2LinkModel.id,
            anchor: {
                name: 'connectionRatio',
                args: { ratio: .28 } // 👈 new ratio here
            }
        },
        labels: [
            {
                range: { min: 0, max: 1 },
                attrs: { labelText: { text: 'IPDA' } },
                position: { distance: 0.5, angle: 10 }
            }
        ]
    });
}