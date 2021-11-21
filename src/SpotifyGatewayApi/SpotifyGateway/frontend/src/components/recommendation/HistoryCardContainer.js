import React, {Component} from 'react';
import {recommendPrefix} from "../../infrastructure/constants/application-constants";
import {connect} from "react-redux";
import Grid from "@material-ui/core/Grid";
import PlaylistCard from "../playlists/PlaylistCard";
import PlaylistResult from "./PlaylistResult";
import SplitPane from "react-split-pane";
import CustomRedirect from "../helpers/CustomRedirect";
import {defaultBackgroundColor, defaultColor, white} from "../../infrastructure/constants/color-constants";

class HistoryCardContainer extends Component {

    state = {
        playlistId: null
    };

    componentDidMount() {
        this.setState({
            playlistId: this.props.playlists && this.props.playlists.length > 0
                ? this.props.playlists[0].id
                : null
        });
    }

    handleSelect = id => {
        this.setState({
            playlistId: id
        });
    };

    render() {
        return this.props.playlists && this.props.playlists.length > 0
            ? (
                <SplitPane style={{backgroundColor: defaultBackgroundColor, height: "max-content"}}
                           split="vertical"
                           minSize="50%">
                    <Grid container spacing={24} style={{marginLeft: '3%'}}>
                        {this.props.playlists.map(playlist => {

                            playlist.color = playlist.id === this.state.playlistId
                                ? defaultColor
                                : white;

                            return (
                                <Grid item xs={12} sm={8} lg={4} xl={3} key={playlist.id} style={{padding: '30px'}}>
                                    <PlaylistCard playlist={playlist} handleSelect={this.handleSelect}/>
                                </Grid>
                            )
                        })}
                    </Grid>
                    {
                        this.state.playlistId
                            ? <PlaylistResult playlistId={this.state.playlistId}/>
                            : null
                    }
                </SplitPane>
            )
            : <CustomRedirect message="User has no recommended playlists. Redirecting to homepage."/>;
    }
}

const mapStateToProps = state => {
    return {
        playlists: state.playlist.userPlaylists.filter(playlist => playlist.name.startsWith(recommendPrefix) && playlist.trackCount > 0)
    };
};

export default connect(mapStateToProps)(HistoryCardContainer);
