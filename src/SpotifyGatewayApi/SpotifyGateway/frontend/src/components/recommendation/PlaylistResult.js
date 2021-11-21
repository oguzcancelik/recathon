import React from 'react';

const PlaylistResult = props => {
    return (
        <div style={{textAlign: "center", marginBottom: '1%', marginTop: '1%'}}>
            <iframe src={`https://open.spotify.com/embed/playlist/${props.playlistId}`} width="600" title={props.playlistId}
                    height="1000" frameBorder="0" allowtransparency="true" allow="encrypted-media"/>
        </div>
    );
};

export default PlaylistResult;
