import React, {Component} from 'react';
import {connect} from "react-redux";
import Loading from "../helpers/Loading";
import Error from "../helpers/Error";
import {Redirect} from "react-router-dom";
import PlaylistResult from "./PlaylistResult";
import Grid from "@material-ui/core/Grid";
import PlaylistCard from "../playlists/PlaylistCard";
import SplitPane from "react-split-pane";
import {addPlaylist} from "../../store/actions/playlistActions";
import {defaultCoverImagePath} from "../../infrastructure/constants/application-constants";
import * as api from "../../infrastructure/api/api-gateway";
import {defaultErrorMessage} from "../../infrastructure/constants/error-constants";
import {defaultBackgroundColor} from "../../infrastructure/constants/color-constants";
import {logout} from "../../store/actions/authActions";

class Recommend extends Component {

    state = {
        loading: true,
        playlist: null,
        errorMessage: null
    }

    componentDidMount() {
        if (!this.props.selectedPlaylistId || !this.props.playlists) {

            this.setState({
                loading: false,
                playlist: null
            });

            return;
        }

        const data = JSON.stringify({
            playlistId: this.props.selectedPlaylistId,
            isBrowser: true
        });

        api.recommend(data)
            .then(res => {
                if (res.data && res.data.result) {

                    let playlist = res.data.result;

                    if (!playlist.imagePath) {
                        playlist.imagePath = defaultCoverImagePath;
                    }

                    this.setState({
                        playlist: playlist
                    });

                    this.props.addPlaylist(playlist);
                }

                this.setState({
                    loading: false
                });
            })
            .catch(error => {

                if (error.response.status === 401) {
                    this.props.logout();
                }
                
                this.setState({
                    loading: false,
                    errorMessage: error.response.data.errors.length && error.response.data.errors[0].userFriendlyMessage
                        ? error.response.data.errors[0].userFriendlyMessage
                        : defaultErrorMessage
                });
            });
    }

    render() {
        return (
            <SplitPane style={{backgroundColor: defaultBackgroundColor, textAlign: "center", height: "max-content"}}
                       split="vertical"
                       minSize="40%">
                <Grid container style={{textAlign: "center", marginLeft: '2%'}}>
                    {
                        this.props.selectedPlaylistId && this.props.playlists
                            ? (
                                <Grid item xs={12} sm={12} lg={8} xl={8} key={this.props.selectedPlaylistId}
                                      style={{padding: '10px', margin: '15%'}}>
                                    <PlaylistCard
                                        playlist={this.props.playlists.find(playlist => playlist.id === this.props.selectedPlaylistId)}
                                    />
                                </Grid>
                            )
                            : null
                    }
                </Grid>

                {
                    this.state.loading
                        ? <Loading text="Generating Your Playlist"/>
                        : this.state.playlist && this.props.playlists
                        ? <PlaylistResult playlistId={this.state.playlist.id}/>
                        : this.state.errorMessage
                            ? <Error message={this.state.errorMessage}/>
                            : <Redirect to="/"/>
                }
            </SplitPane>
        );
    }
}

const mapStateToProps = state => {
    const playlists = state.playlist.userPlaylists && state.playlist.browsePlaylists
        ? state.playlist.userPlaylists.concat(state.playlist.browsePlaylists)
        : state.playlist.userPlaylists
            ? state.playlist.userPlaylists
            : state.playlist.browsePlaylists
                ? state.playlist.browsePlaylists
                : null;

    return {
        playlists: playlists,
        selectedPlaylistId: state.playlist.selectedPlaylistId
    };
};


const mapDispatchToProps = dispatch => {
    return {
        addPlaylist: playlist => dispatch(addPlaylist(playlist)),
        logout: () => dispatch(logout())
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(Recommend);
